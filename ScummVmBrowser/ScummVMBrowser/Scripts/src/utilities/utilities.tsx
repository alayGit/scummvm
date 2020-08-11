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


export function DecodeYEncode(source: string) {
	var
		output = []
		, ck = false
		, i = 0
		, c
		;
	let sourceArray = Array.from(source);
	for (i = 0; i < sourceArray.length; i++) {
		c = sourceArray[i].charCodeAt(0);
		// ignore newlines
		if (c === 13 || c === 10) { continue; }
		// if we're an "=" and we haven't been flagged, set flag
		if (c === 61 && !ck) {
			ck = true;
			continue;
		}
		if (ck) {
			ck = false;
			c = c - 64;
		}
		if (c < 42 && c >= 0) {
			output.push(c + 214);
		} else {
			output.push(c - 42);
		}
	}
	return output;
};