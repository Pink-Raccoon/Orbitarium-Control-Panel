function setUpDisplay(width, height){
	$("#menu > nav > ul > li:nth-child(1) > a").click();
	$("#checkboxClientResolution").prop("checked", false);
	$("#checkboxFullscreen").prop("checked", false);
	$("#checkboxKVM").prop("checked", false);
	$("#customResX").prop("value", width);
	$("#customResY").prop("value", height);
	$("#customResolution").change();
	$("#savePanel > a.applysettings.input-button-large.inline").click();
	$("#settingsChangedReload > div > div > a.input-button-large.success.reconnect").click();
	$("#menu > nav > ul > li:nth-child(2) > a").click();
}