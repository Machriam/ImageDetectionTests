export function DrawSourceImage(guid, data) {
    document.getElementById(guid).getContext("2d", { willReadFrequently: true });
    const canvas = document.getElementById(guid);
    const image = document.createElement("img");
    image.src = data;
    image.onload = (evt) => {
        const data = cv.imread(image);
        cv.imshow(canvas, data);
    };
}
export function Canny(sourceGuid, destGuid, params) {
    InvokeStep(sourceGuid, destGuid, (src, dest) => {
        cv.Canny(src, dest, params[0], params[1], 3, false);
    });
}

export function MedianBlur(sourceGuid, destGuid, params) {
    InvokeStep(sourceGuid, destGuid, (src, dest) => {
        cv.medianBlur(src, dest, params[0]);
    });
}

function InvokeStep(sourceGuid, destGuid, modifyImage) {
    document.getElementById(destGuid).getContext("2d", { willReadFrequently: true });
    const src = cv.imread(sourceGuid);
    const dest = new cv.Mat();
    modifyImage(src, dest);
    cv.imshow(destGuid, dest);
    src.delete();
    dest.delete();
}
function _measureTime(name, func) {
    const start = Date.now();
    func();
    const end = Date.now();
    if (window.state == undefined) window.state = {};
    if (!window.state.hasOwnProperty(name)) window.state[name] = 0;
    state[name] = state[name] + end - start;
    console.log(name + " " + (state[name]));
}