import React, { useState } from 'react';

function Counter() {

    const [state, setState] = useState({ currentCount: 0 });

    const incrementCounter = () =>
        setState({
            ...state,
            currentCount: state.currentCount + 1
        });

    return (
        <div>
            <h1>Counter</h1>

            <p>This is a simple example of a React component.</p>

            <p aria-live="polite">Current count: <strong>{state.currentCount}</strong></p>

            <button className="btn btn-primary" onClick={incrementCounter}>Increment</button>
        </div>
    );
}

export default Counter;
