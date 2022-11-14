import React, { useState, useCallback } from "react";

function useDisplay(initialState) {
    const [state, setState] = useState(initialState);

    // onClick
    const onClick = useCallback(() => {
        setState(prev => !prev);
    }, []);

    return [state, onClick];
}


export default useDisplay;