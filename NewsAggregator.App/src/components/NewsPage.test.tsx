import React from "react";
import { createGetNewsHandler } from "../mocks/handlers";
import NewsPage from "./NewsPage";
import { render } from "../utils/render";
import { screen, waitFor, waitForElementToBeRemoved } from "@testing-library/react";
import { NewsMock } from "../mocks/mocks";

vi.mock("../utils/auth", () => new Promise((resolve, _) => resolve({
    useToken: () => "unit test token",
})))

describe("<NewsPage />", () => {
    it("should load a news entry", async () => {
        // Arrange
        const newsEntry = NewsMock.build()
        const mock = createGetNewsHandler({ items: [newsEntry] })

        // Act
        render(<NewsPage />, { mocks: [mock] })
        const progressbar = await waitFor(() => {
            const progressbar = screen.queryByRole("progressbar")
            expect(progressbar).toBeInTheDocument()
            return progressbar
        })
        await waitForElementToBeRemoved(progressbar)

        // Assert
        expect(screen.getByText(newsEntry.title!)).toBeInTheDocument()
    })
})