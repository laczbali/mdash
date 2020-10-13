import { VpnClient } from './VpnClient';

export class Vpn {
    updatedTime: Date;
    clientList: VpnClient[];
    sumTrafficMbytes: number;
}
