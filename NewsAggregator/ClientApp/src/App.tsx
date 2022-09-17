import React from "react";
import { Route, Routes } from "react-router-dom";
import Layout from "./components/Layout";
import { ToolBarProvider } from "./components/ToolBarProvider";
import NotFound from "./components/NotFound";
import NewsPage from "./components/NewsPage";

function App() {
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
