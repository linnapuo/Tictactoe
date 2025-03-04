import { Alert, Snackbar } from "@mui/material";
import { createSlice, PayloadAction, SerializedError } from "@reduxjs/toolkit";
import { useAppDispatch, useAppSelector } from "src/app/hooks";

export interface ErrorState {
  current: SerializedError | undefined;
  queue: SerializedError[];
}

const initialState: ErrorState = {
  current: undefined,
  queue: [],
};

export const errorSlice = createSlice({
  name: "error",
  initialState,
  reducers: {
    setError: (draft, action: PayloadAction<SerializedError>) => {
      if (draft.current) {
        draft.queue.push(action.payload);
      } else {
        draft.current = action.payload;
      }
    },
    clearError: (draft) => {
      draft.current = draft.queue.pop();
    },
  },
});

export function useErrorHandler() {
  const dispatch = useAppDispatch();

  return (error: string | unknown) => {
    if (typeof error === "string") {
      dispatch(errorSlice.actions.setError({ message: error }));
      return;
    }
    if (typeof error === "object" && error && "message" in error && typeof error.message === "string") {
      dispatch(errorSlice.actions.setError({ message: error.message }));
    }
  };
}

export const errorReducer = errorSlice.reducer;

export function RenderError() {
  const currentMessage = useAppSelector((state) => state.error.current?.message);
  const dispatch = useAppDispatch();

  return (
    <Snackbar open={!!currentMessage} autoHideDuration={6000} onClose={() => dispatch(errorSlice.actions.clearError())}>
      <Alert severity="error" sx={{ width: "100%" }}>
        {currentMessage}
      </Alert>
    </Snackbar>
  );
}
