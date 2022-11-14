let MainId = 0;
document.getElementById("addr").innerHTML += '<u>' + address; +'</u>';
document.getElementById("pub_key").innerHTML += '<u>' + pkey; +'</u>';
document.getElementById("sec_key").innerHTML += '<u>' + skey; +'</u>';

document.addEventListener("click", function (e) {
	if (e.target.id === "noclip")
		return;
	if (e.target.id == "nextImg") { MainId += 1; GetNextImg(e.target); return; }
	if (e.target.id == "prevImg") { MainId -= 1;; GetNextImg(e.target); return; }
	if (e.target instanceof HTMLImageElement && e.target.id !== "zoomimg" && e.target.id == "point") {
		zoomImage(e.target);
	} else {
		if (e.target.localName=="td") unzoomImage();
	}
});
function unzoomImage() {
	document.getElementById("zoom").style.display = "none";
	MainId = 0;
}

function GetNextImg(e) {
	document.getElementById("prevImg").hidden = (MainId == 0);
	document.getElementById("nextImg").hidden = (MainId == json.CollNFT.length - 1);
	let MainImgAlt = (MainId < json.CollNFT.length ? json.CollNFT[MainId] : 0);
	let PrevImgAlt = (MainId > 0 ? json.CollNFT[MainId - 1] : null);
	let NextImgAlt = (MainId < json.CollNFT.length - 1 ? json.CollNFT[MainId + 1] : null);
	e.alt = JSON.stringify(MainImgAlt);
	if (MainId > 0) {
		document.getElementById("prevImg").src = PrevImgAlt.PathNFT;
		document.getElementById("prevImg").alt = JSON.stringify(PrevImgAlt);
	}
	if (MainId < json.CollNFT.length - 1) {
		document.getElementById("nextImg").src = NextImgAlt.PathNFT;
		document.getElementById("nextImg").alt = JSON.stringify(NextImgAlt);
	}
	document.getElementById("cost").innerHTML = MainImgAlt.cost;
	document.getElementById("author").innerHTML = json.Creator.OwnAddrWallet;
	document.getElementById("owner").innerHTML = MainImgAlt.Owner.OwnAddrWallet;
	document.getElementById("MainImage").src = MainImgAlt.PathNFT;
}

function zoomImage(e) {
	let element = document.getElementById("zoom");
	let zoomImage = document.getElementById("zoomImage");
	zoomImage.firstChild.remove();
	let image = document.createElement("img");
	image.src = e.src;
	image.id = "MainImage";
	image.style.borderStyle = "groove";
	image.style.borderColor = "rgba(80, 80, 80, 1.0)";
	image.style.borderWidth = "thick";
	image.style.position = "static";
	zoomImage.appendChild(image);
	element.style.display = "block";
	image.style.width = "800px";
	image.style.height = "800px";
	json = JSON.parse(e.alt);
	document.getElementById("prevImg").hidden = (MainId == 0);
	document.getElementById("nextImg").hidden = (MainId == json.CollNFT.length-1);
	let MainImgAlt = (MainId < json.CollNFT.length ? json.CollNFT[MainId] : 0);
	let PrevImgAlt = (MainId > 0 ? json.CollNFT[MainId - 1] : null);
	let NextImgAlt = (MainId < json.CollNFT.length - 1 ? json.CollNFT[MainId + 1] : null);
	image.alt = JSON.stringify(MainImgAlt);
	if (MainId > 0) {
		document.getElementById("prevImg").src = PrevImgAlt.PathNFT;
		document.getElementById("prevImg").alt = JSON.stringify(PrevImgAlt);
	}
	if (MainId < json.CollNFT.length - 1) {
		document.getElementById("nextImg").src = NextImgAlt.PathNFT;
		document.getElementById("nextImg").alt = JSON.stringify(NextImgAlt);
	}

	document.getElementById("cost").innerHTML = MainImgAlt.cost;
	document.getElementById("author").innerHTML = json.Creator.OwnAddrWallet;
	document.getElementById("owner").innerHTML = MainImgAlt.Owner.OwnAddrWallet;
}
