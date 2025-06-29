import React from "react";
import { AppBar, Box, Fab, Toolbar } from "@mui/material";
import { Outlet } from "react-router-dom";
import KeyboardArrowUpIcon from "@mui/icons-material/KeyboardArrowUp";
import RefreshIcon from "@mui/icons-material/Refresh";
import styled from "@emotion/styled";
import { useToolBar } from "./ToolBarProvider";

const StyledFab = styled(Fab)({
  top: -64,
  margin: '0 auto',
});

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
      </Box>
      <AppBar
        component="nav"
        position="fixed"
        color="transparent"
        sx={{top: 'auto', bottom: -56, boxShadow: 'none'}}>
        <Toolbar>
          <StyledFab color="primary" onClick={onRefresh} disabled={isRefreshPending}>
            <RefreshIcon />
          </StyledFab>
          <Box flexGrow={1} />
          <StyledFab color="primary" onClick={onScrollToTop}>
            <KeyboardArrowUpIcon />
          </StyledFab>
        </Toolbar>
      </AppBar>
    </React.Fragment>
  );
};

export default Layout;
