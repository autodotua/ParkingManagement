import Cookies from "js-cookie"
import { Notification } from "element-ui"
export function withToken(obj: object): object {
    console.log(Cookies.get("token"))
    const request = {
        UserID: Number.parseInt(Cookies.get("userID") ?? "0"),
        Token: Cookies.get("token")
    }
    Object.assign(request, obj);
    console.log("send request", request);
    return request;
}

export function formatDateTime(time: Date): string {
    return time.getFullYear().toString().padStart(4, '0') + "-"
        + time.getMonth().toString().padStart(2, '0') + "-"
        + time.getDay().toString().padStart(2, '0') + " "
        + time.getHours().toString().padStart(2, '0') + ":"
        + time.getMinutes().toString().padStart(2, '0');
}

export function getUrl(controller: string, action: string) {
    return `http://localhost:8520/${controller}/${action}`;
}

export function showError(r: any) {
    Notification.error({ title: "错误", message: r });
    console.log(r);
}