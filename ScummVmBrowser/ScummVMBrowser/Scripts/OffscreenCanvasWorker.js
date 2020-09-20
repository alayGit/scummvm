
self.onmessage = function (evt) {
	console.log("In worker offscreen");
	var offScreenCanvasManager = new OffScreenCanvasManager(evt.data.offScreenCanvas);
	offScreenCanvasManager.Init();
	evt.data.port.onmessage = function (e) {
		offScreenCanvasManager.UpdateCanvas(e.data);
	}

};

const NoBytesPerPixel = 4;
const Width = 320;
const Height = 200;
class OffScreenCanvasManager {
	constructor(canvas) {
		this.Canvas = canvas;
	}

	Init() {
		const ctx = this.Canvas.getContext('2d');
		this.ImageData = ctx.createImageData(Width, Height);
	}

	UpdateCanvas(frames) {
		const ctx = this.Canvas.getContext('2d');

		for (let i = 0; i < frames.length; i++) {
			let byteNo = 0;
			const frame = frames[i];

			for (let heightCounter = frame.Y; heightCounter < frame.H + frame.Y; heightCounter++) {
				for (let widthCounter = frame.X; widthCounter < frame.W + frame.X; widthCounter++) {
					for (let colorComponent = 0; colorComponent < NoBytesPerPixel; colorComponent++) {
						this.ImageData.data[heightCounter * NoBytesPerPixel * Width + widthCounter * NoBytesPerPixel + colorComponent] = frame.Buffer[byteNo + colorComponent];
					}
					byteNo += NoBytesPerPixel;
				}
			}
		}

		ctx.putImageData(this.ImageData, 0, 0);
	}
}

