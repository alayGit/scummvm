export async function GetResponseFromServer<T>(urlPortion:string, responseType: ServerResponseType) {
    const promise = (url: string): Promise<T> => {
        return fetch(url)
            .then(response => {
                if (!response.ok) {
                    throw new Error(response.statusText)
                }
                switch (responseType) {
                    case 'json':
                        return response.json();
                    case 'blob':
                        return response.blob();
                    case 'text':
                        return response.text();
                    case 'nothing':
                        return true;
                    default:
                        throw new Error(`Unknown response type ${responseType}`);
            }
            });
    }
    let returnResult: T = undefined;
    await promise(`${window.location.origin}/${urlPortion}`).then(function (result) {
        returnResult = result;
    });

    return returnResult;
};

type ServerResponseType = 'json' | 'blob' | 'text' | 'nothing';
