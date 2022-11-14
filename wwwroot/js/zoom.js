document.addEventListener("click", function (e) {
	if (e.target.id === "noclip")
		return;
	if ((e.target.id == "next") || e.target.id == "prev") {
		window.location.assign("/Main/Index?vm.number=" + e.accessKey);
	}
	if (e.target instanceof HTMLImageElement && e.target.id !== "zoomimg" && e.target.id == "point") {
		zoomImage(e.target);
	} else {
		if (e.target.localName == "td")unzoomImage();
	}
});
function unzoomImage() {
	document.getElementById("zoom").style.display = "none";
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
	image.alt = e.alt;
	document.getElementById("buy").style.visibility = JSON.parse(image.alt).author.OwnAddrWallet == address ? "hidden" : "visible";
	document.getElementById("cost").innerHTML = JSON.parse(image.alt).cost;
	document.getElementById("author").innerHTML = JSON.parse(image.alt).author.OwnAddrWallet;
	document.getElementById("owner").innerHTML = JSON.parse(image.alt).owner.OwnAddrWallet;
}