"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
// 
const SampleRate = 16000;
const MinQueuedToResumeAudio = 20;
const MaxQueuedToStopAudio = 25;
const FeedSize = 5;
function GetJoinedArrays(numberToJoin, startingPoint, arraysList) {
    let result;
    result = [];
    for (let i = startingPoint; i < numberToJoin; i++) {
        result = result.concat(arraysList[i]);
    }
    return result;
}
class WebAudioStreamer {
    constructor(startSoundReceivingCallback, stopSoundReceivingCallback) {
        this.audioContextOptions = {
            sampleRate: SampleRate
        };
        this.audioBuffer = [];
        this.nextTime = 0;
        this.numberScheduled = 0;
        this.startSoundReceivingCallback = startSoundReceivingCallback;
        this.stopSoundReceivingCallback = stopSoundReceivingCallback;
        this.startSoundReceiving();
        this.context = new (window.AudioContext)(this.audioContextOptions);
    }
    pushOntoAudioBuffer(encodedBytes) {
        if (this.context.state == "suspended") {
            this.context.resume();
        }
        if (this.context.state != "suspended") {
            const streamer = this;
            this.audioBuffer.push(encodedBytes);
            if (this.soundRunning && this.numberScheduled * FeedSize >= MaxQueuedToStopAudio) {
                this.stopSoundReceiving();
            }
            if (this.audioBuffer.length >= FeedSize) {
                const encodedBuffer = new Uint8ClampedArray(GetJoinedArrays(FeedSize, 0, this.audioBuffer)).buffer;
                this.audioBuffer = this.audioBuffer.slice(FeedSize);
                streamer.context.decodeAudioData(encodedBuffer, function (decodedBuffer) {
                    streamer.scheduleBuffers(streamer, decodedBuffer);
                });
            }
        }
    }
    scheduleBuffers(streamer, audioBuffer) {
        var source = streamer.context.createBufferSource();
        source.buffer = audioBuffer;
        source.connect(streamer.context.destination);
        if (streamer.nextTime == 0)
            streamer.nextTime = streamer.context.currentTime; /// add 50ms latency to work well across systems - tune streamer if you like
        source.start(streamer.nextTime);
        streamer.nextTime += source.buffer.duration; // Make the next buffer wait the length of the last buffer before being played
        streamer.numberScheduled++;
        source.onended = function () {
            streamer.numberScheduled--;
            if (!streamer.soundRunning && streamer.numberScheduled * FeedSize <= MinQueuedToResumeAudio) {
                streamer.startSoundReceiving();
            }
        };
    }
    startSoundReceiving() {
        this.startSoundReceivingCallback();
        this.soundRunning = true;
    }
    stopSoundReceiving() {
        this.stopSoundReceivingCallback();
        this.soundRunning = false;
    }
}
exports.WebAudioStreamer = WebAudioStreamer;
//# sourceMappingURL=webAudioStreamer.js.map