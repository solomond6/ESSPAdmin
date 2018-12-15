(function (){

	addEvent(window, "load", function (){
		if (isInternetExplorer())
			polyfillDataUriDownload();
	});

	function polyfillDataUriDownload(){
		var links = document.querySelectorAll('a[download], area[download]');
		for (var index = 0, length = links.length; index<length; ++index) {
			(function (link){
				var dataUri = link.getAttribute("href");
				var fileName = link.getAttribute("download");
				if (dataUri.slice(0,5) != "data:")
					throw new Error("The XHR part is not implemented here.");
				addEvent(link, "click", function (event){
					cancelEvent(event);
					try {
						var dataBlob = dataUriToBlob(dataUri);
						forceBlobDownload(dataBlob, fileName);
					} catch (e) {
						alert(e)
					}
				});
			})(links[index]);
		}
	}

	function forceBlobDownload(dataBlob, fileName){
		window.navigator.msSaveBlob(dataBlob, fileName);
	}

	function dataUriToBlob(dataUri) {
		if  (!(/base64/).test(dataUri))
			throw new Error("Supports only base64 encoding.");
		var parts = dataUri.split(/[:;,]/),
			type = parts[1],
			binData = atob(parts.pop()),
			mx = binData.length,
			uiArr = new Uint8Array(mx);
		for(var i = 0; i<mx; ++i)
			uiArr[i] = binData.charCodeAt(i);
		return new Blob([uiArr], {type: type});
	}

	function addEvent(subject, type, listener){
		if (window.addEventListener)
			subject.addEventListener(type, listener, false);
		else if (window.attachEvent)
			subject.attachEvent("on" + type, listener);
	}

	function cancelEvent(event){
		if (event.preventDefault)
			event.preventDefault();
		else
			event.returnValue = false;
	}

	function isInternetExplorer(){
		return /*@cc_on!@*/false || !!document.documentMode;
	}

})();