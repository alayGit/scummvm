var canvas = document.getElementsByTagName("Canvas")[0];
if (canvas != undefined) {
    return canvas.getContext('2d').getImageData(0, 0, 320, 200);
}
else {
    return null;
}


//modifiedCanvasPutImageData = function (data, dx, dy) {
//    canvasPutImageData(this,data, dx, dy);
//};

//document.getElementsByTagName("Canvas")[0].getContext('2d').putImageData = modifiedCanvasPutImageData;
//alert('Hello World');


