export interface User {
  token: string;
  refreshToken: string;
  user: {
      id: number;
      firstName: string;
      lastName: string;
      username: string;
      email: string;
      password: string;
      phoneNumber: string;
  };
}