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

export function Erode(sourceGuid, destGuid, params) {
    InvokeStep(sourceGuid, destGuid, (src, dest) => {
        const matrix = params[0].flatMap(p => p);
        const kernel = cv.matFromArray(params[0].length, params[0].length, cv.CV_8U, matrix);
        let anchor = new cv.Point(-1, -1);
        cv.erode(src, dest, kernel, anchor, params[1], cv.BORDER_CONSTANT, cv.morphologyDefaultBorderValue());
        kernel.delete();
    });
}

export function Dilate(sourceGuid, destGuid, params) {
    InvokeStep(sourceGuid, destGuid, (src, dest) => {
        const matrix = params[0].flatMap(p => p);
        const kernel = cv.matFromArray(params[0].length, params[0].length, cv.CV_8U, matrix);
        let anchor = new cv.Point(-1, -1);
        cv.dilate(src, dest, kernel, anchor, params[1], cv.BORDER_CONSTANT, cv.morphologyDefaultBorderValue());
        kernel.delete();
    });
}

export function KernelFiltering(sourceGuid, destGuid, params) {
    InvokeStep(sourceGuid, destGuid, (src, dest) => {
        let matrix = params[0].flatMap(p => p);
        const useGrayScale = params[1];
        const normalizeMatrix = params[2];
        if (useGrayScale) cv.cvtColor(src, src, cv.COLOR_RGB2GRAY);
        if (normalizeMatrix) {
            const sum = matrix.reduce((a, b) => a + b, 0);
            if (sum != 0) matrix = matrix.map(x => x / sum);
        }
        const kernel = cv.matFromArray(params[0].length, params[0].length, cv.CV_32FC1, matrix);
        let anchor = new cv.Point(-1, -1);
        cv.filter2D(src, dest, cv.CV_8U, kernel, anchor, 0, cv.BORDER_DEFAULT);
        kernel.delete();
    });
}
export function FindContours(sourceGuid, destGuid, params) {
    document.getElementById(destGuid).getContext("2d", { willReadFrequently: true });
    const src = cv.imread(sourceGuid);
    const dest = cv.Mat.zeros(src.rows, src.cols, cv.CV_8UC3);
    cv.cvtColor(src, src, cv.COLOR_RGBA2GRAY, 0);
    cv.threshold(src, src, 0, 255, cv.THRESH_BINARY);
    let contours = new cv.MatVector();
    let hierarchy = new cv.Mat();
    cv.findContours(src, contours, hierarchy, cv.RETR_CCOMP, cv.CHAIN_APPROX_SIMPLE);
    console.log("Found Contours: " + contours.size());
    let contourAreas = [];
    for (let i = 0; i < contours.size(); ++i) {
        let color = new cv.Scalar(Math.round(Math.random() * 255), Math.round(Math.random() * 255),
            Math.round(Math.random() * 255));
        cv.drawContours(dest, contours, i, color, 1, cv.LINE_8, hierarchy, 100);
        contourAreas.push(cv.contourArea(contours.get(i)));
    }
    console.log("Snowmans: " + contourAreas.filter(a => a > 350).length);
    console.log("Trees: " + contourAreas.filter(a => a < 350).length);
    contours.delete(); hierarchy.delete();
    cv.imshow(destGuid, dest);
    src.delete();
    dest.delete();
}
export function FindWaldo(sourceGuid, destGuid, params) {
    document.getElementById(destGuid).getContext("2d", { willReadFrequently: true });
    const src = cv.imread(sourceGuid);
    const dest = new cv.Mat();
    const waldoTemplate = "data:image/jpeg;base64,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";
    debugger;
    const image = document.createElement("img");
    image.src = waldoTemplate;
    src.copyTo(dest);
    image.onload = (evt) => {
        const waldoData = cv.imread(image);
        cv.matchTemplate(dest, waldoData, dest, cv.TM_CCORR_NORMED);
        cv.imshow(destGuid, dest);
        src.delete();
        dest.delete();
    };
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
export function Invert(sourceGuid, destGuid, params) {
    InvokeStep(sourceGuid, destGuid, (src, dest) => {
        cv.cvtColor(src, src, cv.COLOR_RGBA2GRAY, 0);
        cv.bitwise_not(src, dest);
    });
}
export function PowerLaw(sourceGuid, destGuid, params) {
    InvokeStep(sourceGuid, destGuid, (src, dest) => {
        cv.convertScaleAbs(src, dest, params[0], params[1]);
    });
}
export function Threshold(sourceGuid, destGuid, params) {
    InvokeStep(sourceGuid, destGuid, (src, dest) => {
        cv.threshold(src, dest, params[0], params[1], cv.THRESH_BINARY);
    });
}
export function AdaptiveThreshold_Mean(sourceGuid, destGuid, params) {
    InvokeStep(sourceGuid, destGuid, (src, dest) => {
        cv.cvtColor(src, src, cv.COLOR_RGBA2GRAY, 0);
        const thresholdType = params[3] ? cv.THRESH_BINARY_INV : cv.THRESH_BINARY;
        cv.adaptiveThreshold(src, dest, params[0], cv.ADAPTIVE_THRESH_MEAN_C, thresholdType, params[1], params[2]);
    });
}
export function AdaptiveThreshold_Gaussian(sourceGuid, destGuid, params) {
    InvokeStep(sourceGuid, destGuid, (src, dest) => {
        cv.cvtColor(src, src, cv.COLOR_RGBA2GRAY, 0);
        const thresholdType = params[3] ? cv.THRESH_BINARY_INV : cv.THRESH_BINARY;
        cv.adaptiveThreshold(src, dest, params[0], cv.ADAPTIVE_THRESH_GAUSSIAN_C, thresholdType, params[1], params[2]);
    });
}

export function FourierTransform(sourceGuid, destGuid, params) {
    InvokeStep(sourceGuid, destGuid, (src, dest) => {
        cv.cvtColor(src, src, cv.COLOR_RGBA2GRAY, 0);
        let optimalRows = cv.getOptimalDFTSize(src.rows);
        let optimalCols = cv.getOptimalDFTSize(src.cols);
        let s0 = cv.Scalar.all(0);
        let padded = new cv.Mat();
        cv.copyMakeBorder(src, padded, 0, optimalRows - src.rows, 0,
            optimalCols - src.cols, cv.BORDER_CONSTANT, s0);
        let plane0 = new cv.Mat();
        padded.convertTo(plane0, cv.CV_32F);
        let planes = new cv.MatVector();
        let complexI = new cv.Mat();
        let plane1 = new cv.Mat.zeros(padded.rows, padded.cols, cv.CV_32F);
        planes.push_back(plane0);
        planes.push_back(plane1);
        cv.merge(planes, complexI);
        cv.dft(complexI, complexI);
        cv.split(complexI, planes);
        cv.magnitude(planes.get(0), planes.get(1), planes.get(0));
        let mag = planes.get(0);
        let m1 = new cv.Mat.ones(mag.rows, mag.cols, mag.type());
        cv.add(mag, m1, mag);
        cv.log(mag, mag);
        // crop the spectrum, if it has an odd number of rows or columns
        let rect = new cv.Rect(0, 0, mag.cols & -2, mag.rows & -2);
        mag = mag.roi(rect);
        // rearrange the quadrants of Fourier image
        // so that the origin is at the image center
        let cx = mag.cols / 2;
        let cy = mag.rows / 2;
        let tmp = new cv.Mat();

        let rect0 = new cv.Rect(0, 0, cx, cy);
        let rect1 = new cv.Rect(cx, 0, cx, cy);
        let rect2 = new cv.Rect(0, cy, cx, cy);
        let rect3 = new cv.Rect(cx, cy, cx, cy);

        let q0 = mag.roi(rect0);
        let q1 = mag.roi(rect1);
        let q2 = mag.roi(rect2);
        let q3 = mag.roi(rect3);
        // exchange 1 and 4 quadrants
        q0.copyTo(tmp);
        q3.copyTo(q0);
        tmp.copyTo(q3);

        // exchange 2 and 3 quadrants
        q1.copyTo(tmp);
        q2.copyTo(q1);
        tmp.copyTo(q2);

        // The pixel value of cv.CV_32S type image ranges from 0 to 1.
        cv.normalize(mag, dest, 0, 1, cv.NORM_MINMAX);
        padded.delete(); planes.delete(); complexI.delete(); m1.delete(); tmp.delete(); mag.delete(); plane0.delete(); plane1.delete();
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