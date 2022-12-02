import React from "react";
import { Box, CircularProgress, Grid } from "@mui/material";
import { useGetNews } from "../openapi/backendComponents";
import NewsCard from "./NewsCard";
import { useToolBarDispatch } from "./ToolBarProvider";

const NewsPage: React.FC = () => {
  const [isLatest, setIsLatest] = React.useState<boolean>(false);
  const { data, isLoading, error, mutate } = useGetNews();
  const newsEntries = data ?? [];

  const toolBarDispatch = useToolBarDispatch();

  React.useEffect(() => {
    mutate({
      body: {
        isLatest: isLatest,
      },
    });
  }, [isLatest, mutate]);

  React.useEffect(() => {
    const onRefresh = async () => {
      if (!isLatest) {
        setIsLatest(true);
      } else {
        mutate({
          body: {
            isLatest: isLatest,
          },
        });
      }
    };

    toolBarDispatch({
      onRefresh,
    });
  }, [toolBarDispatch, isLatest, mutate]);

  if (isLoading) {
    return (
      <Box
        display="flex"
        minHeight="50vh"
        justifyContent="center"
        alignItems="center"
      >
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return <>Error: {JSON.stringify(error)}</>;
  }

  return (
    <Grid container spacing={2} justifyContent="center">
      {newsEntries.map((news, i) => (
        <Grid item key={i}>
          <NewsCard news={news} />
        </Grid>
      ))}
    </Grid>
  );
};

export default NewsPage;
