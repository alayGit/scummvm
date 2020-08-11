
offScreenCanvasManager = undefined;
onmessage = function (evt) {
    if (evt.data.offScreenCanvas != undefined) {
        offScreenCanvasManager = new OffScreenCanvasManager(evt.data.offScreenCanvas);
        offScreenCanvasManager.Init();
    }
    else {
        offScreenCanvasManager.UpdateCanvas(evt.data.frames);
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
            const frameBuffer = DecodeYEncode(frame.Buffer);
            for (let heightCounter = frame.Y; heightCounter < frame.H + frame.Y; heightCounter++) {
                for (let widthCounter = frame.X; widthCounter < frame.W + frame.X; widthCounter++) {
                    for (let colorComponent = 0; colorComponent < NoBytesPerPixel; colorComponent++) {
                        this.ImageData.data[heightCounter * NoBytesPerPixel * Width + widthCounter * NoBytesPerPixel + colorComponent] = frameBuffer[byteNo + colorComponent];
                    }
                    byteNo += NoBytesPerPixel;
                }
            }
        }

        ctx.putImageData(this.ImageData, 0, 0);
    }
}

function DecodeYEncode(source) {
    var
        output = []
        , ck = false
        , i = 0
        , c
        ;
    let sourceArray = Array.from(source);
    for (i = 0; i < sourceArray.length; i++) {
        c = sourceArray[i].charCodeAt(0);
        // ignore newlines
        if (c === 13 || c === 10) { continue; }
        // if we're an "=" and we haven't been flagged, set flag
        if (c === 61 && !ck) {
            ck = true;
            continue;
        }
        if (ck) {
            ck = false;
            c = c - 64;
        }
        if (c < 42 && c >= 0) {
            output.push(c + 214);
        } else {
            output.push(c - 42);
        }
    }
    return output;
};

