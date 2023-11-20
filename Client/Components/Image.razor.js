export function DrawSourceImage(guid, data) {
    const canvas = document.getElementById(guid);
    const image = document.createElement("img");
    image.src = data;
    image.onload = (evt) => {
        const data = cv.imread(image);
        cv.imshow(canvas, data);
    };
}
export function Canny(sourceGuid, destGuid, params) {
    const src = cv.imread(sourceGuid);
    const dest = new cv.Mat();
    cv.Canny(src, dest, params[0], params[1], 3, false);
    cv.imshow(destGuid, dest);
    src.delete();
    dest.delete();
}