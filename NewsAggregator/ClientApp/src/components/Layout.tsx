import React from "react";
import { AppBar, Box, IconButton, Toolbar } from "@mui/material";
import { Outlet } from "react-router-dom";
import KeyboardArrowUpIcon from "@mui/icons-material/KeyboardArrowUp";
import RefreshIcon from "@mui/icons-material/Refresh";
import { useToolBar } from "./ToolBarProvider";

const Layout = () => {
  const [isRefreshPending, setIsRefreshPending] =
    React.useState<boolean>(false);
  const toolBar = useToolBar();

  const onScrollToTop = () => {
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const onRefresh = () => {
    setIsRefreshPending(true);

    toolBar.onRefresh().finally(() => {
      setIsRefreshPending(false);
    });
  };

  return (
    <React.Fragment>
      <Box component="main">
        <Outlet />
        <Toolbar />
      </Box>
      <AppBar component="nav" position="fixed" sx={{ top: "auto", bottom: 0 }}>
        <Toolbar>
          <IconButton
            color="inherit"
            onClick={onRefresh}
            disabled={isRefreshPending}
          >
            <RefreshIcon />
          </IconButton>
          <Box sx={{ flexGrow: 1 }} />
          <IconButton color="inherit" onClick={onScrollToTop}>
            <KeyboardArrowUpIcon />
          </IconButton>
        </Toolbar>
      </AppBar>
    </React.Fragment>
  );
};

export default Layout;
