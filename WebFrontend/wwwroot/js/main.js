// Highlight Current Page Link
// ------------------------------------------------------------------- //
let currentLink = getLink("/" + window.location.pathname.split('/')[1]);
console.log("/" + window.location.pathname.split('/')[1]);
function getLink(currentPage) {
	let links = document.getElementsByClassName("page-item");
	for (let l of links) {
		console.log(l.getAttribute("href"));

		if(l.getAttribute("href") == currentPage)
		{
			return l;
		}
    };
    return null;
}
if(currentLink != null) {
	currentLink.style.opacity = "1";
}

// Get Color Theme
// ------------------------------------------------------------------- //
document.onload = getTheme();

function getTheme() {
	document.body.className = localStorage.getItem('theme');
	document.getElementById("noise").className = localStorage.getItem('theme');

	if (document.body.className === "light") {
		document.body.style.setProperty('--primaryAccent', '#4bbdd9');
		document.body.style.setProperty('--whiteText', '#555555');
		document.body.style.setProperty('--greyText', '#4A4A4A');
		document.body.style.setProperty('--inactiveLight', '#888888');
		document.body.style.setProperty('--inactiveDark', '#AAAAAA');
		document.body.style.setProperty('--leftGradient', 'linear-gradient(-200deg, #B3B3B3, #D6D6D6 85%)');
		document.body.style.setProperty('--rightGradient', 'linear-gradient(-200deg, #A3A3A3, #DAD9D9 85%)');
	}
	else
	{
		document.body.style.setProperty('--primaryAccent', '#4bbdd9');
		document.body.style.setProperty('--whiteText', '#f5f5f5');
		document.body.style.setProperty('--greyText', '#b5b5b5');
		document.body.style.setProperty('--inactiveLight', '#777777');
		document.body.style.setProperty('--inactiveDark', '#555555');
		document.body.style.setProperty('--leftGradient', 'linear-gradient(-200deg, #4C4C4C, #292929 85%)');
		document.body.style.setProperty('--rightGradient', 'linear-gradient(-200deg, #5C5C5C, #252626 85%)');
	}
}

// Noise Generation DO NOT DELETE
// ------------------------------------------------------------------- //
// $('#noise').noisy({
//     intensity: 3,
//     size: 800,
//     opacity: 0.2,
//     monochrome: true,
//     randomColors: false,
//     color: '#ffffff'
// });
