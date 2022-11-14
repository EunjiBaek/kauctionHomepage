import { EAuctionKind } from '../types/enums.js';

export class ImagePathProcessor {
    static NO_IMAGE_PATH = '/img/list_noimg.jpg';

    processAuctionImage(fileName, aucKind, aucNum, forceReplace = true) {
        if (!fileName) {
            return ImagePathProcessor.NO_IMAGE_PATH;
        }
        return fileName.startsWith('https://') 
            ? fileName 
            : `${ImagePathProcessor.#getAuctionFolderName(aucKind, aucNum)}/${forceReplace ? ImagePathProcessor.#replaceFileName(fileName) : fileName}`;
    }
    
    processConsignmentImage(fileName) {
        return fileName ? `${ImagePathProcessor.#getConsignmentFolderName()}/${fileName}` : ImagePathProcessor.NO_IMAGE_PATH;
    }

    static #getAuctionFolderName(aucKind, aucNum) {
        return $.cardUtils.getImgFolder(aucKind, aucNum, aucKind === EAuctionKind.majorAbsentee ? '/T' : '');
    }
    
    static #getConsignmentFolderName() {
        return `${$.cardUtils.getImageDomain()}/www/Consign`
    }

    static #replaceFileName(fileName) {
        return fileName.replace('.jpg', '_L.jpg');
    }
}
