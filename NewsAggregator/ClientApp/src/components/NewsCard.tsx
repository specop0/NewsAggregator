import React from "react";
import {
  Card,
  CardActionArea,
  CardContent,
  CardHeader,
  CardMedia,
  Link,
  Typography,
} from "@mui/material";
import { News } from "../openapi/backendSchemas";

type NewsCardProps = {
  news: News;
};

const NewsCard: React.FC<NewsCardProps> = ({ news }) => {
  return (
    <Card sx={{ maxWidth: 768 }}>
      <CardActionArea component={Link} href={news.url ?? ""} target="_blank" rel="noopener noreferrer">
        <CardHeader title={news.title} />
        <CardMedia component="img" image={news.imageData ?? ""} />
        <CardContent>
          <Typography>{news.summary}</Typography>
        </CardContent>
      </CardActionArea>
    </Card>
  );
};

export default NewsCard;
