interface WindowEnv {
  env?: {
    apiUrl?: string;
    apiVer?: number;
  };
}

declare const window: WindowEnv;

export const environment = {
  production: false,
  api:{
    url: window?.env?.apiUrl ?? '',
    ver: window?.env?.apiVer ?? 1
  },  
  name: "Refrigeraciones Dante",
};