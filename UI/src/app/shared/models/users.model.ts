export interface RegisterModel {
    UserName?: string;
    Email?: string;
    PasswordHash?: string;
}

export interface LoginModel {
    Email?: string;
    PasswordHash?: string;
}
