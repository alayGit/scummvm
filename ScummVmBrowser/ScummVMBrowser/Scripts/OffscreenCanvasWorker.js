
self.onmessage = function (evt) {
	var offScreenCanvasManager = new OffScreenCanvasManager(evt.data.offScreenCanvas);
	offScreenCanvasManager.Init();
	evt.data.toOffScreenCanvasWorker.onmessage = function (e) {
		offScreenCanvasManager.UpdateCanvas(e.data);
	}

};

const NoBytesPerPixel = 4;
const Width = 320;
const Height = 200;
class OffScreenCanvasManager {
	constructor(canvas) {
		this.Canvas = canvas;
		this.PaletteMap = new Map();
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
			if (frame.PaletteBuffer) {
				this.PaletteMap[frame.PaletteHash] = frame.PaletteBuffer;
			}

			for (let heightCounter = frame.Y; heightCounter < frame.H + frame.Y; heightCounter++) {
				for (let widthCounter = frame.X; widthCounter < frame.W + frame.X; widthCounter++) {
					if (frame.PictureBuffer[byteNo] !== frame.IgnoreColour || frame.ignoreColour === -1)
						for (let colorComponent = 0; colorComponent < NoBytesPerPixel; colorComponent++) {
							this.ImageData.data[heightCounter * NoBytesPerPixel * Width + widthCounter * NoBytesPerPixel + colorComponent] = this.PaletteMap[frame.PaletteHash][frame.PictureBuffer[byteNo]][colorComponent];
						}
					byteNo++;
				}
			}
		}

		ctx.putImageData(this.ImageData, 0, 0);
	}

}

