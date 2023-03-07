import { recoverTypedSignature } from '@metamask/eth-sig-util';
import { Contract } from "@ethersproject/contracts";
import { Web3Provider, getDefaultProvider, JsonRpcProvider } from "@ethersproject/providers";
import { BigNumber } from "ethers";



export async function recover(data, signature) {
    
    return await recoverTypedSignature({ data: data, signature: signature, version: "V1" });
}

export function getWeb3(){
    return window.ethereum.send({method:"eth_accounts"}).result[0];
    
}

export default function getLibrary(provider, chainId) {
    const currentChainId = typeof provider.chainId === "number"
        ? provider.chainId
        : typeof provider.chainId === "string"
            ? parseInt(provider.chainId)
            : "any";

    if (currentChainId != chainId && chainId == 250)
        return new JsonRpcProvider("https://rpc.ftm.tools");

    if (currentChainId != chainId && chainId == 4002)
        return new JsonRpcProvider("https://rpc.testnet.fantom.network/");


    const library = new Web3Provider(
        provider,
        currentChainId
    );
    library.pollingInterval = 15_000;

    return library;
}

function getSigner(library, account) {
    return library.getSigner(account).connectUnchecked();
}

// account is optional
function getProviderOrSigner(library, account) {
    return account ? getSigner(library, account) : library;
}

export function getContract(address, ABI, account, chainId = 250){
    //var lib = getLibrary(window.ethereum);
    //console.log(lib);
    var providerOrSigner = getProviderOrSigner(getLibrary(window.ethereum, chainId), account);
    //console.log(providerOrSigner);
    
    return new Contract(address, ABI, providerOrSigner);
}
const BatcherAddress = "0x6c926e3723E1B33A294a1F1042D3FE8444A2C368";
export const SwapAddress = "0x1eE483EC364037A3B50c16D430eb7C77f5c0f7dc";
const BalanceAddress = "0x786c676d805c82d8458daf2991f332d6c7b3a686";
export const MarketplaceAddress = "0xCd594B4Ea6059bE5F99839eB39dd4404cDa9829d";

export function getBatcherContract(abi,account, chainId = 250){
    let providerOrSigner = getProviderOrSigner(getLibrary(window.ethereum, chainId), account);
    return new Contract(BatcherAddress, abi, providerOrSigner);
}

export function getSwapContract(abi, account){
    let providerOrSigner = getProviderOrSigner(getLibrary(window.ethereum), account);
    return new Contract(SwapAddress, abi, providerOrSigner);
}

export function getBalanceContract(abi, account){
    let providerOrSigner = getProviderOrSigner(getLibrary(window.ethereum), account);
    return new Contract(BalanceAddress, abi, providerOrSigner);
}

export function getMarketplaceContract(abi, account) {
    let providerOrSigner = getProviderOrSigner(getLibrary(window.ethereum), account);
    return new Contract(MarketplaceAddress, abi, providerOrSigner);
}

export async function removeAnimateTransform(tokenURIImage){
    let svg = Buffer.from(tokenURIImage.substring(26), "base64").toString();
    let image = svg;
    let d = document.createElement("document");
    let parser = new DOMParser();
    let doc = parser.parseFromString(image, "image/svg+xml");

    let tags = Array.prototype.slice.call(doc.getElementsByTagName("animateTransform"));
    for (const tag of tags) {
        tag.remove();
    }
    d.appendChild(doc.documentElement);
    let buf = Buffer.from(d.innerHTML);
    return "data:image/svg+xml;base64,"+buf.toString("base64");
}

export const toHex = (n, numBytes = 0) => {
    const asHexString = BigNumber.isBigNumber(n)
        ? n.toHexString().slice(2)
        : typeof n === "string"
            ? hexRegex.test(n)
                ? n.replace(/0x/, "")
                : (+n).toString(16)
            : (+n).toString(16);
    return `0x${asHexString.padStart(numBytes * 2, "0")}`;
};
export const toBN = (n) => BigNumber.from(toHex(n));