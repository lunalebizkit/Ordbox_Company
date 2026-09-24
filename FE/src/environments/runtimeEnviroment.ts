import { environment } from './environment';

export const runtimeEnvironment = {
  apiUrl: (window as any).env?.apiUrl ?? environment.api.url,
  apiVer: (window as any).env?.apiVer ?? environment.api.ver
};
