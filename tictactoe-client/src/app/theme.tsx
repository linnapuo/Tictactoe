import { createTheme, IconButton, PaletteMode, ThemeProvider, useMediaQuery, useTheme } from "@mui/material";
import React, { createContext, FC, useContext, useEffect, useMemo, useState } from "react";
import Brightness4Icon from '@mui/icons-material/Brightness4';
import Brightness7Icon from '@mui/icons-material/Brightness7';

interface Ctx {
    setColorMode: (mode: PaletteMode) => void
}

interface Props {
    children: React.ReactNode
}

const ColorModeContext = createContext<Ctx>({ setColorMode: () => { throw new Error("setColorMode not set"); } });

export const AppThemeProvider: FC<Props> = ({children}) => {
    const prefersDarkMode = useMediaQuery("(prefers-color-scheme: dark)");
    const [mode, setMode] = useState<PaletteMode>(prefersDarkMode ? "dark" : "light");

    const colorMode = useMemo(() => ({
        setColorMode: (mode: PaletteMode) => {
            setMode(mode);
        }
    }), []);

    const theme = useMemo(() =>
        createTheme({
            palette: {
                mode: mode
            }
        }), 
    [mode]);
    
    useEffect(() => {
        colorMode.setColorMode(prefersDarkMode ? "dark" : "light");
    }, [colorMode, prefersDarkMode]);

    return (
        <ColorModeContext.Provider value={colorMode}>
            <ThemeProvider theme={theme}>
                {children}
            </ThemeProvider>
        </ColorModeContext.Provider>
    );
}


export const ToggleColorModeButton = () => {
    const {palette: { mode }} = useTheme();
    const setColorMode = useContext(ColorModeContext).setColorMode;

    const isDarkMode = (mode: PaletteMode) => mode === "dark";
    const toggleMode = (mode: PaletteMode) => isDarkMode(mode) ? "light" : "dark";

    return (
        <IconButton onClick={() => { setColorMode(toggleMode(mode)); }}>
            {isDarkMode(mode) ? <Brightness7Icon/> : <Brightness4Icon/>}
        </IconButton>
    );
}