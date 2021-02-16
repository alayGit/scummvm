import * as React from "react";
import { useState, useEffect, useRef } from 'react';
//Thanks to https://overreacted.io/making-setinterval-declarative-with-react-hooks/
export function useInterval(callback : () => void, delay: number) {
	const savedCallback = useRef();

	// Remember the latest callback.
	useEffect(() => {
		(savedCallback as any).current = callback;
	}, [callback]);

	// Set up the interval.
	useEffect(() => {
		function tick() {
			(savedCallback as any).current();
		}
		if (delay !== null) {
			let id = setInterval(tick, delay);
			return () => clearInterval(id);
		}
	}, [delay]);
}
