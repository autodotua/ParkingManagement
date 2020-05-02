
import Cookies from "js-cookie"
export function withToken(obj: object): object {
    console.log(Cookies.get("token"))
    const request = {
        UserID: Number.parseInt(Cookies.get("userID") ?? "0"),
        Token: Cookies.get("token")
    }
    console.log(request);
    Object.assign(request, obj);
    console.log(request);
    return request;
}