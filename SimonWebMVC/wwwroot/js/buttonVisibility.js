function changeElementVisibility(elementId, isAuthorized) {
	var element = document.getElementById(elementId);
	if (isAuthorized) {
		element.style.visibility == 'visible'	
	} else {
		element.style.visibility = 'hidden'
	}
}