export const getCookies = (name: string): string | undefined => {
    const match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
    if (match) {
        return match[2];
    }
    return undefined;
};