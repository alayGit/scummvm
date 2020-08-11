"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const NoBytesPerPixel = 4;
const Width = 320;
const Height = 200;
class OffScreenCanvasManager {
    constructor(canvas) {
        this.Canvas = canvas;
        this.Init = this.Init;
        this.UpdateCanvas = this.UpdateCanvas;
    }
    Init() {
        const ctx = this.Canvas.getContext('2d');
        this.ImageData = ctx.createImageData(Width, Height);
    }
    UpdateCanvas(frame) {
        let byteNo = 0;
        const ctx = this.Canvas.getContext('2d');
        for (let heightCounter = frame.y; heightCounter < frame.h + frame.y; heightCounter++) {
            for (let widthCounter = frame.x; widthCounter < frame.w + frame.x; widthCounter++) {
                for (let colorComponent = 0; colorComponent < NoBytesPerPixel; colorComponent++) {
                    this.ImageData.data[heightCounter * NoBytesPerPixel * Width + widthCounter * NoBytesPerPixel + colorComponent] = frame.picBuff[byteNo + colorComponent];
                }
                byteNo += NoBytesPerPixel;
            }
        }
        ctx.putImageData(this.ImageData, 0, 0);
    }
}
exports.OffScreenCanvasManager = OffScreenCanvasManager;
//# sourceMappingURL=offscreenCanvasManager.js.map