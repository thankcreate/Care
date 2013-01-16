var gblPhotoShufflerDivId = "div-gallery";
var gblPhotoShufflerImgId = "img-gallery"; 
var gblImg = new Array(
  "./images/gallery_2.jpg",
  "./images/gallery_3.jpg",
  "./images/gallery_4.jpg",
  "./images/gallery_5.jpg",
  "./images/gallery_1.jpg"
  );
var gblPauseSeconds = 5;
var gblFadeSeconds = .85;
var gblRotations = 1;

var gblDeckSize = gblImg.length;
var gblOpacity = 100;
var gblOnDeck = 0;
var gblStartImg;
var gblImageRotations = gblDeckSize * (gblRotations+1);

window.onload = photoShufflerLaunch;

function photoShufflerLaunch()
{
	var theimg = document.getElementById(gblPhotoShufflerImgId);
	gblStartImg = theimg.src; // save away to show as final image

	document.getElementById(gblPhotoShufflerDivId).style.backgroundImage='url(' + gblImg[gblOnDeck] + ')';
	setTimeout("photoShufflerFade()",gblPauseSeconds*1000);
}

function photoShufflerFade()
{
  var theimg = document.getElementById(gblPhotoShufflerImgId);
	
  // determine delta based on number of fade seconds
  // the slower the fade the more increments needed
  var fadeDelta = 100 / (30 * gblFadeSeconds);

  // fade top out to reveal bottom image
  if (gblOpacity < 2*fadeDelta ) 
  {
    gblOpacity = 100;
    // stop the rotation if we're done
    //if (gblImageRotations < 1) return;

    photoShufflerShuffle();
    // pause before next fade
    setTimeout("photoShufflerFade()",gblPauseSeconds*1000);

  } else  {

    gblOpacity -= fadeDelta;
    setOpacity(theimg,gblOpacity);
    setTimeout("photoShufflerFade()",30);  // 1/30th of a second

  }
}

function photoShufflerShuffle()
{
  var thediv = document.getElementById(gblPhotoShufflerDivId);
  var theimg = document.getElementById(gblPhotoShufflerImgId);

  // copy div background-image to img.src
  theimg.src = gblImg[gblOnDeck];
  // set img opacity to 100
  setOpacity(theimg,100);

  // shuffle the deck
  gblOnDeck = ++gblOnDeck % gblDeckSize;
  //alert(gblOnDeck);
  // decrement rotation counter
  //if (--gblImageRotations < 1)
  //{
    // insert start/final image if we're done
   // gblImg[gblOnDeck] = gblStartImg;
  //}

  // slide next image underneath
  thediv.style.backgroundImage='url(' + gblImg[gblOnDeck] + ')';
}

function setOpacity(obj, opacity) {
  opacity = (opacity == 100)?99.999:opacity;
  
  // IE/Win
  obj.style.filter = "alpha(opacity:"+opacity+")";
  
  // Safari<1.2, Konqueror
  obj.style.KHTMLOpacity = opacity/100;

  // Older Mozilla and Firefox
  obj.style.MozOpacity = opacity/100;

  // Safari 1.2, newer Firefox and Mozilla, CSS3
  obj.style.opacity = opacity/100;
}
