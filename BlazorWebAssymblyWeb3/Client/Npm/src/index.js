import {
    getBalanceContract,
    getDefaultWeb3,
    getBatcherContract,
    getContract,
    getSwapContract,
    getWeb3,
    recover,
    removeAnimateTransform,
    SwapAddress,
    getMarketplaceContract,
    MarketplaceAddress
} from './recover_lib';

import YokaiHeroesAbi from './YokaiChainAbi.json';
import BatcherAbi from './BatcherAbi.json';
import SwapAbi from './SwapAbi.json';
import BalanceAbi from './BalanceAbi.json';
import NftKeyAbi from './NftKeyAbi.json';
import MarketplaceAbi from './MarketplaceAbi.json';
import WFTMAbi from './wftm.json';
import { utils, BigNumber, Signer } from "ethers";
import { _TypedDataEncoder } from "@ethersproject/hash";

export async function Recover(data, signature) {
    console.log('hash: ' + signature);
    console.log(data);
    return await recover(data, signature);
}

export function ready() {
    const el = document.querySelector(".gallery-tool-bar-c");
    if (el) {
        const observer = new IntersectionObserver(
            ([e]) => { e.target.classList.toggle("is-pinned", e.intersectionRatio < 1); },
            {
                threshold: [1],
                rootMargin: '-1px 0px 0px 0px',
            }
        );
        
        observer.observe(el);
    }
}


let web3Provider = null;
let contract = null;
let addressOfNFTcontract = "";

function setContractAndProvider(addressOfNFT, chainId = 250){
    addressOfNFTcontract = addressOfNFT;
    web3Provider = getWeb3();
    contract = getContract(addressOfNFT, YokaiHeroesAbi, web3Provider, chainId);
}

export async function GetTotalSupply(addressOfNFT, chainId = 250){
    if(contract == null || addressOfNFTcontract !== addressOfNFT)
        setContractAndProvider(addressOfNFT, chainId);

    let total;
    try {
        total = (await contract.totalSupply()).toNumber();
    }
    catch (ex) {
        return -1;
    }
    console.log(total);
    return total;
}

//0x1A7d6ed890b6C284271AD27E7AbE8Fb5211D0739
export async function GetTokenNFTKeyListing(addressOfNFT, tokenId) {
    const cont = getContract("0x1A7d6ed890b6C284271AD27E7AbE8Fb5211D0739", NftKeyAbi, web3Provider);

    const tx = await cont.getTokenListing(addressOfNFT, tokenId);
    const value = JSON.stringify({ TokenId: tx[0].toString(), Price: tx[1].toString(), expireTimestamp: tx[3].toString(), Address: addressOfNFT });
    console.log(value);
    return value;
}

export async function GetTokenNFTKeyListings(addressOfNFT, from, size) {
    const cont = getContract("0x1A7d6ed890b6C284271AD27E7AbE8Fb5211D0739", NftKeyAbi, web3Provider);

    const tx = await cont.getTokenListings(addressOfNFT, from, size);
    let listings = [];
    for (let t of tx) {
        if (t[0] == 0) continue;
        listings.push({ TokenId: t[0].toString(), Price: t[1].toString(), expireTimestamp: t[3].toString(), Address: addressOfNFT });
    }
    //const value = JSON.stringify(listings);
    //console.log(value);
    return listings;
}

export async function OwnerOf(addressOfNFT, tokenId){
    if(contract == null || addressOfNFTcontract !== addressOfNFT)
        setContractAndProvider(addressOfNFT);
    
    let ownerAddress = await contract.ownerOf(tokenId);
    return ownerAddress;
}

export async function TransferFrom(addressOfNFT, addressTo, tokenId){
    try {
        let tx = await contract.transferFrom(web3Provider, addressTo, tokenId);
        tx.wait().then(async transaction => {
            console.log(transaction.status);
        });
    }catch(ex){
        
    }
}

export async function BlazorScrollToId(id) {
    const element = document.getElementById(id);
    if (element instanceof HTMLElement) {
        element.scrollIntoView(true);
    }
}

export async function GetBalanceOflistedOf(userAddress){
    const account = getWeb3();
    const balanceHelper = getBalanceContract(BalanceAbi, account);
    const balances = await balanceHelper.getBalanceOf(userAddress);
    
    let balancesParsed = [];
    for(let balance of balances){
        if(parseInt(balance[1]) === 0) continue;
        balancesParsed.push({CollectionAddress:balance[0],Balance:balance[1].toNumber()})
    }
    
    console.log(balancesParsed);
    return balancesParsed;
}

export async function GetOwnedNFTIdOfCollection(addressOfNFT, userAddress = ""){
    if(contract == null || addressOfNFTcontract !== addressOfNFT)
        setContractAndProvider(addressOfNFT);
    if(userAddress === "")
        userAddress = web3Provider;
    const batcher = getBatcherContract(BatcherAbi,web3Provider);
    let idsBN = await batcher.getIds(addressOfNFT, userAddress);
    let ids = [];
    
    for(let idBn of idsBN){
        ids.push(idBn.toNumber());
    }
    console.log(ids);
    return ids;
}

//* Swap Offers *//
export async function IsSwapContractApproved(addressOfNFT, tokenID){
    if(contract == null || addressOfNFTcontract !== addressOfNFT)
        setContractAndProvider(addressOfNFT);

    return (await contract.getApproved(tokenID)).toLowerCase() === SwapAddress.toLowerCase();
}

export async function ApproveSwapContractForTokenID(addressOfNFT, tokenID, componentInstance) {
    if(contract == null || addressOfNFTcontract !== addressOfNFT)
        setContractAndProvider(addressOfNFT);
    
    try {
        const tx = await contract.approve(SwapAddress, tokenID);
        await componentInstance.invokeMethodAsync("WaitingTransaction", "Approve");
        const receipt = await tx.wait();
        await componentInstance.invokeMethodAsync("ApproveResult", receipt.status === 1, "");
    }
    catch (ex){
        await componentInstance.invokeMethodAsync("ApproveResult", false, ex.message);
    }
}

export async function RevokeSwapContractForTokenID(addressOfNFT, tokenID, componentInstance) {
    if(contract == null || addressOfNFTcontract !== addressOfNFT)
        setContractAndProvider(addressOfNFT);

    try {
        const tx = await contract.approve("0x0000000000000000000000000000000000000000", tokenID);
        await componentInstance.invokeMethodAsync("WaitingTransaction", "Revoke");

        const receipt = await tx.wait();
        await componentInstance.invokeMethodAsync("RevokeResult", receipt.status === 1, "");
    }
    catch (ex){
        await componentInstance.invokeMethodAsync("RevokeResult", false, ex.message);
    }
}

export async function AcceptOffer(bidder, targetCollection, offerCollection, targetID, offerID, end, componentInstance) {
    const web3Provider = getWeb3();
    const contract = getSwapContract(SwapAbi, web3Provider);
    
    try {
        const tx = await contract.acceptOffer([bidder, targetCollection, offerCollection, targetID, offerID, end]);
        await componentInstance.invokeMethodAsync("WaitingTransaction", "Accept");

        const receipt = await tx.wait();
        await componentInstance.invokeMethodAsync("AcceptOfferResult",receipt.status === 1, receipt.transactionHash);
    }
    catch (ex){
        await componentInstance.invokeMethodAsync("AcceptOfferResult", false, "");
    }
}

export async function CancelOffer(bidder, targetCollection, offerCollection, targetID, offerID, end, componentInstance) {
    const web3Provider = getWeb3();
    const contract = getSwapContract(SwapAbi, web3Provider);

    const tx = await contract.cancelOffer([bidder,targetCollection,offerCollection,targetID,offerID,end]);
    //await componentInstance.invokeMethodAsync("WaitingTransaction", "Cancel");
    
    const receipt = await tx.wait();
    if(receipt.status === 1)
        await componentInstance.invokeMethodAsync("CancelOfferResult");
}

export async function CreateOffer(bidder, targetCollection, offerCollection, targetID, offerID, end, componentInstance){
    const web3Provider = getWeb3();
    const contract = getSwapContract(SwapAbi, web3Provider);
    
    try {
        const tx = await contract.createOffer([bidder, targetCollection, offerCollection, targetID, offerID, end], {value: "1000000000000000000"});
        await componentInstance.invokeMethodAsync("WaitingTransaction", "Create");

        const receipt = await tx.wait();
        await componentInstance.invokeMethodAsync("CreateResult", receipt.status === 1);
    }
    catch (ex){
        await componentInstance.invokeMethodAsync("CreateResult", false, ex.message);
    }
}

export async function HideTransactionState(componentInstance, shouldReload = false){
    setTimeout(async () =>{
        await componentInstance.invokeMethodAsync("Hide", shouldReload);
    }, 2000)
}

export async function GetTokenUri(batcherAddres, addressOfNFT, ids, slice = 5, chainId = 250) {
    if (contract == null || addressOfNFTcontract !== addressOfNFT)
        setContractAndProvider(addressOfNFT, chainId);


    let tokenURIsTemp = [];
    //for (var id of ids)
    //{
    //    tokenURIsTemp.push(await contract.tokenURI(id));
    //}

    
    //let tokenURIs = [];
    const batcher = getBatcherContract(BatcherAbi, web3Provider, batcherAddres, chainId);
    

    const nbOfIdToRetrieve = Math.floor(ids.length / slice);
    const lastNbToRetrieve = ids.length % slice;
    for (let i = 0; i < nbOfIdToRetrieve; i++) {
        const sliced = ids.slice(slice * i, (slice * (i + 1)));
        let toAdd = await batcher.getTokenURIOf(addressOfNFTcontract, sliced);
        tokenURIsTemp = tokenURIsTemp.concat(toAdd);
    }

    if (lastNbToRetrieve !== 0) {
        const sliced = ids.slice(-lastNbToRetrieve);
        const abd = await batcher.getTokenURIOf(addressOfNFTcontract, sliced);
        tokenURIsTemp = tokenURIsTemp.concat(abd);
    }
    
    // for(let tokenUri of tokenURIsTemp){
    //     if(isValidHttpUrl(tokenUri)){
    //         let response = await fetch(tokenUri);
    //         if(response.ok)
    //             tokenURIs.push(await response.text());
    //     }else if(tokenUri.startsWith("ipfs://")){
    //         let idIPFS = tokenUri.substring(6);
    //         let response = await fetch(tokenUri);
    //         if(response.ok)
    //             tokenURIs.push(await response.text());
    //     }else if(tokenUri.length !== 0){
    //         let bufJson = Buffer.from(tokenUri.substring(29), "base64");
    //         let jsonObj = JSON.parse(bufJson.toString());
    //         tokenURIs.push(JSON.stringify(jsonObj));
    //     }
    //     else{
    //         tokenURIs.push("");
    //     }
    // }

    return tokenURIsTemp;
    //return tokenURIs;
}

export async function GetTokenUriOf(tokenMeta){
    if(isValidHttpUrl(tokenMeta)){
        let response = await fetch(tokenMeta);
        if(response.ok)
            return await response.text();
    }else if(tokenMeta.startsWith("ipfs://")){
        let idIPFS = tokenMeta.substring(6);
        let response = await fetch("https://ipfs.io/ipfs/" + idIPFS);
        if(response.ok)
            return await response.text();
    }else if(tokenMeta.length !== 0){
        let bufJson = Buffer.from(tokenMeta.substring(29), "base64");
        let jsonObj = JSON.parse(bufJson.toString());
        return JSON.stringify(jsonObj);
    }
    else{
        return "";
    }
}

function isValidHttpUrl(string) {
    let url;

    try {
        url = new URL(string);
    } catch (_) {
        return false;
    }

    return url.protocol === "http:" || url.protocol === "https:";
}

export async function RemoveAnimateTransform(imageString){
    return await removeAnimateTransform(imageString);
}

export async function requestPermissions() {
    console.log('requestPermissions');

    if (ethereum.chainId == null)
        return false;

    var result = await ethereum
        .request({
            method: 'wallet_requestPermissions',
            params: [{ eth_accounts: {} }],
        });
    return true;
}

export async function copyText(text) {
    navigator.clipboard.writeText(text).then(function () {
      ;
    })
        .catch(function (error) {
            alert(error);
        });
}

export async function getBalance(address) {
    const balance = await ethereum.request({ method: "eth_getBalance", params: [address] })
    
    console.log(balance);
    return balance;
}

//* MARKETPLACE *//
const HashZero = "0x0000000000000000000000000000000000000000000000000000000000000000";
const AddressZero = "0x0000000000000000000000000000000000000000";
let marketplaceContract = null;
const MAX_BYTES = 65536;
const MAX_UINT32 = 4294967295;
const hexRegex = /[A-Fa-fx]/g;
const toBN = (n) => BigNumber.from(toHex(n));

const toHex = (n, numBytes = 0) => {
    const asHexString = BigNumber.isBigNumber(n)
        ? n.toHexString().slice(2)
        : typeof n === "string"
            ? hexRegex.test(n)
                ? n.replace(/0x/, "")
                : (+n).toString(16)
            : (+n).toString(16);
    return `0x${asHexString.padStart(numBytes * 2, "0")}`;
};
const toKey = (n) => toHex(n, 32);

export async function IsMarketplaceContractApproved(addressOfNFT, tokenID) {
    if (contract == null || addressOfNFTcontract !== addressOfNFT)
        setContractAndProvider(addressOfNFT);

    return (await contract.getApproved(tokenID)).toLowerCase() === MarketplaceAddress.toLowerCase();
}

export async function ApproveMarketplaceContractForTokenID(addressOfNFT, tokenID, componentInstance) {
    if (contract == null || addressOfNFTcontract !== addressOfNFT)
        setContractAndProvider(addressOfNFT);

    try {
        const tx = await contract.approve(MarketplaceAddress, tokenID);
        await componentInstance.invokeMethodAsync("WaitingTransaction", "Approve");
        const receipt = await tx.wait();
        await componentInstance.invokeMethodAsync("ApproveResult", receipt.status === 1, "");
    }
    catch (ex) {
        await componentInstance.invokeMethodAsync("ApproveResult", false, ex.message);
    }
}

const WFTMAddress = "0x21be370d5312f44cb42ce377bc9b8a0cef1a4c83";
export async function IsWFTMApprovedForMarketplace() {
    web3Provider = getWeb3();
    contract = getContract(WFTMAddress, WFTMAbi, web3Provider, 250);
    addressOfNFTcontract = "";
    

    return (await contract.allowance(web3Provider, MarketplaceAddress)) > 0;
}

export async function ApproveWFTMForMarketplace(componentInstance) {
    web3Provider = getWeb3();
    contract = getContract(WFTMAddress, WFTMAbi, web3Provider, 250);
    addressOfNFTcontract = "";
    
    try {
        const tx = await contract.approve(MarketplaceAddress, "999999999000000000000000000");
        await componentInstance.invokeMethodAsync("WaitingTransaction", "Approve");
        const receipt = await tx.wait();
        await componentInstance.invokeMethodAsync("ApproveResult", receipt.status === 1, "");
    }
    catch (ex) {
        await componentInstance.invokeMethodAsync("ApproveResult", false, ex.message);
    }
}

export async function GetWFTMBalance() {
    web3Provider = getWeb3();
    contract = getContract(WFTMAddress, WFTMAbi, web3Provider, 250);
    addressOfNFTcontract = "";

    return await contract.balanceOf(web3Provider);
}

export async function WrapFTM(amount, componentInstance) {
    web3Provider = getWeb3();
    contract = getContract(WFTMAddress, WFTMAbi, web3Provider, 250);
    addressOfNFTcontract = "";
    var value = utils.parseEther(amount.toString());
    try {
        const tx = await contract.deposit({value});
        await componentInstance.invokeMethodAsync("WaitingTransaction");
        const receipt = await tx.wait();
        await componentInstance.invokeMethodAsync("TransactionDone", receipt.status === 1, "");
    }
    catch (ex) {
        await componentInstance.invokeMethodAsync("TransactionDone", false, ex.message);
    }
}

export async function UnwrapFTM(amount, componentInstance) {
    web3Provider = getWeb3();
    contract = getContract(WFTMAddress, WFTMAbi, web3Provider, 250);
    addressOfNFTcontract = "";
    var value = utils.parseEther(amount.toString());
    try {
        const tx = await contract.withdraw(value);
        await componentInstance.invokeMethodAsync("WaitingTransaction");
        const receipt = await tx.wait();
        await componentInstance.invokeMethodAsync("TransactionDone", receipt.status === 1, "");
    }
    catch (ex) {
        await componentInstance.invokeMethodAsync("TransactionDone", false, ex.message);
    }
}

const owner = "0x89B07Ba2d3c04A55632060AA9ea372E1408e3d7B";

export async function createOrderFor(token, tokenId, seller, price, end, royaltyPercent, royaltyAddress, componentInstance) {
    if (marketplaceContract == null)
        marketplaceContract = getMarketplaceContract(MarketplaceAbi, getWeb3());

    if (price <= 0)
        return;

    if (end === 0)
        return;

    const offer = [getOffer(2, token, tokenId)];

    var price = utils.parseEther(price);
    var fee = price.div(50);
    var newprice = price.sub(fee);
    let royalty = 0;
    if (royaltyPercent > 0) {
        royalty = price.div(royaltyPercent);
        newprice = newprice.sub(royalty);
    }
    const consideration = [
        getItemETH(newprice, newprice, seller),
        getItemETH(fee, fee, owner),
        
    ];

    if (royaltyPercent > 0) {
        consideration.push(getItemETH(royalty, royalty, royaltyAddress));
    }
    try {
        const order = await createOrder(
            seller,
            AddressZero,
            offer,
            consideration,
            0,
            end// FULL_OPEN
        );

        var tx = await marketplaceContract.validate([order]);
        await componentInstance.invokeMethodAsync("WaitingTransaction", JSON.stringify(order));
        const receipt = await (await tx).wait();
        //console.log("done");
        await componentInstance.invokeMethodAsync("TransactionDone", receipt.status == 1, "");
    }
    catch (ex) {
        await componentInstance.invokeMethodAsync("TransactionDone", false, ex.message);
    }
}

export async function cancelOfferSeaport(orderComponents, componentInstance){
    if (marketplaceContract == null)
        marketplaceContract = getMarketplaceContract(MarketplaceAbi, getWeb3());

    const components = parseOrder(JSON.parse(orderComponents));

    console.log(components);
    try {
        var tx = await marketplaceContract.cancel([components]);
        await componentInstance.invokeMethodAsync("WaitingTransaction", "");
        const receipt = await (await tx).wait();
        //console.log("done");
        await componentInstance.invokeMethodAsync("TransactionDone", receipt.status == 1, "");
    }
    catch (ex) {
        await componentInstance.invokeMethodAsync("TransactionDone", false, ex.message);
    }
}

export async function createOfferFor(token, tokenId, seller, price, end, componentInstance) {
    if (marketplaceContract == null)
        marketplaceContract = getMarketplaceContract(MarketplaceAbi, getWeb3());

    if (price <= 0)
        return;

    if (end === 0)
        return;

    var price = utils.parseEther(price);
    var fee = price.div(50);
    var newprice = price.sub(fee);

    const offer = [getWFTMOffer(price)];

    const consideration = [
        getOfferConsideration(2, token, tokenId, 1, seller),
        getOfferConsideration(1, WFTMAddress, 0, fee, owner)
    ];
    try {
        const order = await createOrder(
            seller,
            AddressZero,
            offer,
            consideration,
            0,
            end// FULL_OPEN
        );

        var tx = await marketplaceContract.validate([order]);
        await componentInstance.invokeMethodAsync("WaitingTransaction", JSON.stringify(order));
        const receipt = await (await tx).wait();
        await componentInstance.invokeMethodAsync("TransactionDone", receipt.status == 1, "");
    }
    catch (ex) {
        await componentInstance.invokeMethodAsync("TransactionDone", false, ex.message);
    }
}

function parseOrder(orderParameters) {
    for (let a of orderParameters.consideration) {
        a.endAmount = BigNumber.from(a.endAmount);
        a.startAmount = BigNumber.from(a.startAmount);
        a.identifierOrCriteria = BigNumber.from(a.identifierOrCriteria);
    }

    for (let a of orderParameters.offer) {
        a.endAmount = BigNumber.from(a.endAmount);
        a.startAmount = BigNumber.from(a.startAmount);
        a.identifierOrCriteria = BigNumber.from(a.identifierOrCriteria);
    }

    orderParameters.zoneHash = HashZero;
    orderParameters.conduitKey = HashZero;
    orderParameters.totalOriginalConsiderationItems = orderParameters.consideration.length;
    orderParameters.endTime = BigNumber.from(orderParameters.endTime);
    orderParameters.startTime = BigNumber.from(orderParameters.startTime);
    orderParameters.nonce = BigNumber.from(orderParameters.nonce);
    return orderParameters;
}

export async function fulfillOfferOrder(orderjson, signature, componentInstance) {
    if (marketplaceContract == null)
        marketplaceContract = getMarketplaceContract(MarketplaceAbi, getWeb3());

    let orderParameters = JSON.parse(orderjson);
    orderParameters = parseOrder(orderParameters);

    const order = {
        parameters: orderParameters,
        signature: signature,
        numerator: 1,
        denominator: 1,
        extraData: "0x",
    };

    try {
        const tx = await marketplaceContract.fulfillOrder(
            order,
            toKey(false)
        );
        await componentInstance.invokeMethodAsync("WaitingTransaction");
        const receipt = await (await tx).wait();
        console.log(receipt);
        await componentInstance.invokeMethodAsync("TransactionDone", receipt.status == 1, receipt.transactionHash, "");
    }
    catch (ex) {
        await componentInstance.invokeMethodAsync("TransactionDone", false, "", ex.message);
    }
}

export async function fulfillOrder(orderjson, signature, componentInstance) {
    if (marketplaceContract == null)
        marketplaceContract = getMarketplaceContract(MarketplaceAbi, getWeb3());

    let orderParameters = JSON.parse(orderjson);
    orderParameters = parseOrder(orderParameters);

    const value = orderParameters.offer
        .map((x) =>
            x.itemType === 0
                ? x.endAmount.gt(x.startAmount)
                    ? x.endAmount
                    : x.startAmount
                : toBN(0)
        )
        .reduce((a, b) => a.add(b), toBN(0))
        .add(
            orderParameters.consideration
                .map((x) =>
                    x.itemType === 0
                        ? x.endAmount.gt(x.startAmount)
                            ? x.endAmount
                            : x.startAmount
                        : toBN(0)
                )
                .reduce((a, b) => a.add(b), toBN(0))
        );

    const order = {
        parameters: orderParameters,
        signature: signature,
        numerator: 1,
        denominator: 1,
        extraData: "0x",
    };

    try {
        const tx = await marketplaceContract.fulfillOrder(
            order,
            toKey(false),
            { value }
        );
        await componentInstance.invokeMethodAsync("WaitingTransaction");
        const receipt = await (await tx).wait();
        //console.log(receipt);
        await componentInstance.invokeMethodAsync("TransactionDone", receipt.status == 1, receipt.transactionHash, "");
    }
    catch (ex) {
        await componentInstance.invokeMethodAsync("TransactionDone", false, "", ex.message);
    }
}

export async function cancelOrder(orderjson, componentInstance) {
    if (marketplaceContract == null)
        marketplaceContract = getMarketplaceContract(MarketplaceAbi, getWeb3());

    let orderParameters = JSON.parse(orderjson);
    orderParameters = parseOrder(orderParameters);
    try {
        const tx = await marketplaceContract.cancel([orderParameters]);
        await componentInstance.invokeMethodAsync("WaitingTransaction");
        const receipt = await (await tx).wait();
        //console.log(receipt);
        await componentInstance.invokeMethodAsync("TransactionDone", receipt.status == 1,"");
    }
    catch (ex) {
        await componentInstance.invokeMethodAsync("TransactionDone", false, ex.message);
    }

}

function getOfferOrConsiderationItem(itemType, token, tokenId, startAmount, endAmount) {
    return {
        itemType,
        token,
        identifierOrCriteria: tokenId,
        startAmount: startAmount,
        endAmount: endAmount,
    };
}

function getWFTMOffer(amount) {
    const offerItem = getOfferOrConsiderationItem(1, WFTMAddress, 0, amount, amount);
    return offerItem;
}

function getOfferConsideration(itemType, token, tokenId, amount, recipient) {
    const offerItem = getOfferOrConsiderationItem(itemType, token, tokenId, amount, amount);
    return {
        ...offerItem,
        recipient: recipient,
    };
}

function getOffer(itemType, token, tokenId) {
    const offerItem = getOfferOrConsiderationItem(itemType, token, tokenId, 1, 1);

    //const offerItem = {
    //    itemType,
    //    token,
    //    identifierOrCriteria: tokenId,
    //    startAmount: 1,
    //    endAmount: 1,
    //};

    return offerItem;
}

function getConsideration(startAmount, endAmount, recipient) {
    const offerItem = getOfferOrConsiderationItem(0, AddressZero, 0, startAmount, endAmount);
    //const offerItem = {
    //    itemType: 0,
    //    token: AddressZero,
    //    identifierOrCriteria: 0,
    //    startAmount: startAmount,
    //    endAmount: endAmount,
    //};

    return {
        ...offerItem,
        recipient: recipient,
    };
}

function getItemETH(startAmount, endAmount, recipient) {
    return getConsideration(startAmount, endAmount, recipient);
}

async function createOrder(offerer, zone, offer, consideration, orderType, end) {
    const nonce = (await marketplaceContract.getNonce(offerer)).toNumber();
    //get nonce
    const salt = HashZero;//randomHex();
    const startTime = 0;
    const endTime = end;
    const zoneHash = HashZero;
    const conduitKey = HashZero;

    const orderParameters = {
        offerer: offerer,
        zone: zone,
        offer,
        consideration,
        totalOriginalConsiderationItems: consideration.length,
        orderType,
        zoneHash,
        salt,
        conduitKey,
        startTime,
        endTime,
    };

    const orderComponents = {
        ...orderParameters,
        nonce,
    };

    const populated = await _TypedDataEncoder.resolveNames(domainData, types, orderComponents, (name) => {
        if (this.provider == null) {
            logger.throwError("cannot resolve ENS names without a provider", Logger.errors.UNSUPPORTED_OPERATION, {
                operation: "resolveName",
                value: name
            });
        }
        return this.provider.resolveName(name);
    });

    const data = JSON.stringify({
        types: types,
        domain: populated.domain,
        message: populated.value,
        primaryType: "OrderComponents"
    });

    var sig = await ethereum.request({
        method: 'eth_signTypedData_v4',
        params:
            [
                ethereum.selectedAddress,
                data
            ],
        from: ethereum.selectedAddress
    });

    const orderHash = await getAndVerifyOrderHash(orderComponents);
    //console.log(orderHash);
    const order = {
        parameters: orderComponents,
        signature: sig,
        orderHash: orderHash,
        numerator: 1, // only used for advanced orders
        denominator: 1, // only used for advanced orders
        extraData: "0x", // only used for advanced orders
    };

    //console.log(data);
    //console.log(order);
    //console.log(JSON.stringify(order))

    //const { isValidated, isCancelled, totalFilled, totalSize } = await marketplaceContract.getOrderStatus(orderHash);

    return order;
}

export async function getOrderStatus(orderHash) {
    if (marketplaceContract == null)
        marketplaceContract = getMarketplaceContract(MarketplaceAbi, getWeb3());
    return await marketplaceContract.getOrderStatus(orderHash);
}

async function getAndVerifyOrderHash(orderComponents) {
    return await marketplaceContract.getOrderHash(
        orderComponents
    );
}

function randomHex(bytes = 32) { return `0x${randomBytes(bytes)}`; }

function randomBytes(size, cb) {
    // phantomjs needs to throw
    if (size > MAX_UINT32) throw new RangeError('requested too many random bytes')

    var bytes = Buffer.allocUnsafe(size)

    if (size > 0) {  // getRandomValues fails on IE if size == 0
        if (size > MAX_BYTES) { // this is the max bytes crypto.getRandomValues
            // can do at once see https://developer.mozilla.org/en-US/docs/Web/API/window.crypto.getRandomValues
            for (var generated = 0; generated < size; generated += MAX_BYTES) {
                // buffer.slice automatically checks if the end is past the end of
                // the buffer so we don't have to here
                crypto.getRandomValues(bytes.slice(generated, generated + MAX_BYTES))
            }
        } else {
            crypto.getRandomValues(bytes)
        }
    }

    if (typeof cb === 'function') {
        return process.nextTick(function () {
            cb(null, bytes)
        })
    }

    return bytes
}

Object.defineProperties(BigNumber.prototype, {
    toJSON: {
        value: function () {
            return this.toHexString();
        },
    },
});
 
const domainData = {
    name: "Seaport",
    version: "1",
    chainId: 250,
    verifyingContract: MarketplaceAddress,
};

const types = {
    OrderComponents: [
        { name: "offerer", type: "address" },
        { name: "zone", type: "address" },
        { name: "offer", type: "OfferItem[]" },
        { name: "consideration", type: "ConsiderationItem[]" },
        { name: "orderType", type: "uint8" },
        { name: "startTime", type: "uint256" },
        { name: "endTime", type: "uint256" },
        { name: "zoneHash", type: "bytes32" },
        { name: "salt", type: "uint256" },
        { name: "conduitKey", type: "bytes32" },
        { name: "nonce", type: "uint256" },
    ],
    OfferItem: [
        { name: "itemType", type: "uint8" },
        { name: "token", type: "address" },
        { name: "identifierOrCriteria", type: "uint256" },
        { name: "startAmount", type: "uint256" },
        { name: "endAmount", type: "uint256" },
    ],
    ConsiderationItem: [
        { name: "itemType", type: "uint8" },
        { name: "token", type: "address" },
        { name: "identifierOrCriteria", type: "uint256" },
        { name: "startAmount", type: "uint256" },
        { name: "endAmount", type: "uint256" },
        { name: "recipient", type: "address" },
    ],
};