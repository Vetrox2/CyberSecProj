export interface ImageCaptchaDto {
  images: ImageDto[];
  challenge: string;
  encryptedKey: string;
}

export interface ImageDto {
  index: number;
  base64Data: string;
}

export interface VerifyCaptchaDto {
  selectedIndices: number[];
  encryptedKey: string;
}
