import { useAuth } from "react-oidc-context"

export const useToken = (): string => {
    const { user } = useAuth();
    return user?.access_token ?? "";
}