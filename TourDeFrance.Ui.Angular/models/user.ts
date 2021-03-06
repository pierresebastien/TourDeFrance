﻿/// <reference path="../references.ts"/>

class SimpleUser {
    id: string;
    username: string;
    firstName: string;
    lastName: string;
    gender: Gender;
    displayName: string;
    isAdministrator: boolean;
    isBlocked: boolean;
    isDisabled: boolean;
}

class User extends SimpleUser {
    height: number;
    weight: number;
    email: string;
    birthDate: Date;
    apiKey: string;
}

class AuthenticatedUser extends User {
    lastPasswordChangeDate: Date;
    requireNewPasswordAtLogon: boolean;
    numberOfFailedAttempts: number;
}