let addNFT = document.getElementById("addNFT");
let json = "{ }";
let max = 0;

function closeFormAdd() {
    document.getElementById('addCollection').style.display = 'none';
    document.getElementById('visNFT').innerHTML = document.getElementById('nft0').innerHTML + '<div id="nft' + (max + 1) + '"></div>';
    max = 0;
}

document.getElementById("close").addEventListener("click", (e) => {
    closeFormAdd();
})

document.addEventListener("focusout", (e) => {
    if ((e.target instanceof HTMLInputElement) && (e.target.alt!=""))
        document.getElementById("img" + e.target.alt).src = e.target.value;
})

addNFT.addEventListener("click", (e) => {
    let nft = document.getElementById('nft' + max);
    max += 1;
    let nftNext = document.getElementById('nft' + max);
    nftNext.innerHTML = nft.innerHTML;
    nftNext.childNodes[1].id = "url" + max;
    nftNext.childNodes[1].alt = max;
    nftNext.childNodes[5].id = "img" + max;
    nftNext.childNodes[5].alt = max;
    let visNFT = document.getElementById('visNFT');
    let div = document.createElement("div");
    div.id = "nft" + (max + 1);
    visNFT.appendChild(div);
})

document.getElementById("sendCollect").addEventListener("click", (e) => {
    if (confirm("Вы уверены, что хотите загрузить в блокчейн данную коллекцию? (После того, как вы нажмёте ОК, ваша коллекция отправится на обработку сервером. После успешной обработки будет выполнен деплой коллекции. Об успехе или провале создания NFT коллекции вас уведомят через 'Обратную связь')")) {
        let snft0 = "";
        for (let curr = 0; (curr < max + 1); curr++) {
            let elem = document.getElementById('nft' + curr);
            let snft = "";
            for (let i = 0; (i < elem.childNodes.length); i++) {
                if (elem.childNodes[i].id == "url" + curr) url0 = '"' + elem.childNodes[i].value + '"';
                if (elem.childNodes[i].id == "isSelling") isSelling0 = '"' + elem.childNodes[i].checked + '"';
                if (elem.childNodes[i].id == "cost") cost0 = '"' + elem.childNodes[i].value + '"';
                if (elem.childNodes[i].id == "descript") {
                    descript0 = '"' + elem.childNodes[i].value + '"';
                    snft = '"' + curr + '"' + ':{"url":' + url0 + ',"isSelling":'+isSelling0 + ',"cost":' + cost0 + ',"descript":' + descript0 + '}';
                    snft0 += snft + ((curr == max) ? "" : ",");
                    break;
                }
            }
        }
        json = "{" + snft0 + "}";
    
        let Storage = window.localStorage;
        let item = Storage.getItem("PersonLog");
        let pkey = "";
        let skey = "";
        let address = "";
        if (item != null) {
            pkey = JSON.parse(item).public_key;
            skey = JSON.parse(item).secret_key;
            address = JSON.parse(item).address;
            var req = new XMLHttpRequest();
            req.open("POST", "/Main/LoaderNFTofJSON?vm.public_key=" + pkey + "&vm.secret_key=" + skey + "&vm.address=" + address + "&vm.JSONofNFT=" + JSON.stringify(json), false);
            req.send(null);
            if (req.responseText == "True") alert("NFT коллекция отправлена на обработку");
        }

        document.location.reload();
        closeFormAdd();
    }
})