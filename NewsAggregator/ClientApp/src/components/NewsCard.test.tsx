import React from "react";
import { NewsMock } from "../mocks/mocks";
import NewsCard from "./NewsCard";
import { render } from "@testing-library/react";
import { screen } from "@testing-library/react";

describe("<NewsCard />", () => {
    it("should show a news entry", () => {
        // Arrange
        const newsEntry = NewsMock.build()

        // Act
        render(<NewsCard news={newsEntry} />)

        // Assert
        const displayName = `${newsEntry.title} ${newsEntry.summary}`
        const link = screen.getByRole("link", { name: displayName })
        expect(link).toBeInTheDocument()
        expect(link.getAttribute("href")).toBe(newsEntry.url)
    })
})