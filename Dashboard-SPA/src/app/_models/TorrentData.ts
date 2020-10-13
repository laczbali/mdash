export class TorrentData {
    downloads: [{
        name: string,
        progress: number,
        eta: number
    }?];
    numOfUpload: number;
    totalDlSpeed: number;
    totalTorrents: number;
    totalUpSpeed: number;
    querytime: Date;
    maxEta: number;
    uiUrl: string;
}
