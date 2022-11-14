import React from 'react';
import { ToastContainer, Zoom } from 'react-toastify';
import styled from 'styled-components';


const StyledContainer = styled(ToastContainer)`
  .Toastify__close-button {
    display: none;
  }

  .Toastify__toast-icon {
    svg {
      fill: var(--toastify-color-light);
    }
  }

  .Toastify__toast--success {
    background-color: var(--toastify-color-success);

    .Toastify__toast-body {
      color: var(--toastify-color-light);
    }
  }

  .Toastify__toast--error {
    background-color: var(--toastify-color-error);
    .Toastify__toast-body {
      color: var(--toastify-color-light);
    }
  }
`;



const ToastComponent = () => {


    return (
        <StyledContainer
            position="bottom-center"
            autoClose={1000}
            hideProgressBar
            newestOnTop={false}
            closeOnClick={false}
            rtl={false}
            pauseOnFocusLoss={false}
            draggable={false}
            pauseOnHover={false}
            transition={Zoom}
        />
    )
}



export default ToastComponent;
