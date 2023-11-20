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

export function EqualizeGrayHist(sourceGuid, destGuid, params) {
    InvokeStep(sourceGuid, destGuid, (src, dest) => {
        cv.cvtColor(src, src, cv.COLOR_RGBA2GRAY, 0);
        cv.equalizeHist(src, dest);
    });
}

export function EqualizeColorHist(sourceGuid, destGuid, params) {
    InvokeStep(sourceGuid, destGuid, (src, dest) => {
        let hsvPlanes = new cv.MatVector();
        let mergedPlanes = new cv.MatVector();
        cv.cvtColor(src, src, cv.COLOR_RGB2HSV, 0);
        cv.split(src, hsvPlanes);
        let H = hsvPlanes.get(0);
        let S = hsvPlanes.get(1);
        let V = hsvPlanes.get(2);
        cv.equalizeHist(V, V);
        mergedPlanes.push_back(H);
        mergedPlanes.push_back(S);
        mergedPlanes.push_back(V);
        cv.merge(mergedPlanes, src);
        cv.cvtColor(src, dest, cv.COLOR_HSV2RGB, 0);
        hsvPlanes.delete();
        mergedPlanes.delete();
    });
}
export function GaussianBlur(sourceGuid, destGuid, params) {
    InvokeStep(sourceGuid, destGuid, (src, dest) => {
        cv.GaussianBlur(src, dest, new cv.Size(0, 0), params[0], params[1]);
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