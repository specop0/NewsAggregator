import React from "react";
import { Route, Routes } from "react-router-dom";
import Layout from "./components/Layout";
import { ToolBarProvider } from "./components/ToolBarProvider";
import NotFound from "./components/NotFound";
import NewsPage from "./components/NewsPage";
import { useAutoSignin } from "react-oidc-context";
import { Box, CircularProgress, Typography } from "@mui/material";

function App() {
  const { isLoading, isAuthenticated, error } = useAutoSignin({ signinMethod: "signinRedirect" });

  if (isLoading) {
    return (
      <Box>
        <CircularProgress />
      </Box>
    );
  }

  if (!isAuthenticated) {
    return (
      <Box>
        <Typography>Unable to log in.</Typography>
      </Box>
    );
  }

  if (error) {
    return (
      <Box>
        <Typography>Error at log in:</Typography>
        <Typography>{error.message}</Typography>
      </Box>
    );
  }

  return (
    <ToolBarProvider>
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<NewsPage />} />
          <Route path="*" element={<NotFound />}></Route>
        </Route>
      </Routes>
    </ToolBarProvider>
  );
}

export default App;
