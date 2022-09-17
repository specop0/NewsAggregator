import React from "react";

type ToolBarProps = { onRefresh: () => Promise<void> };

const ToolBarContext = React.createContext<ToolBarProps>({
  onRefresh: async () => {},
});
const ToolBarDispatchContext = React.createContext<
  React.Dispatch<React.SetStateAction<ToolBarProps>>
>(() => {});

export const useToolBar = (): ToolBarProps => {
  return React.useContext(ToolBarContext);
};

export const useToolBarDispatch = (): React.Dispatch<
  React.SetStateAction<ToolBarProps>
> => {
  return React.useContext(ToolBarDispatchContext);
};

export const ToolBarProvider: React.FC<React.PropsWithChildren> = ({
  children,
}) => {
  const [toolBar, toolBarDispatch] = React.useState<ToolBarProps>({
    onRefresh: async () => {},
  });

  return (
    <ToolBarContext.Provider value={toolBar}>
      <ToolBarDispatchContext.Provider value={toolBarDispatch}>
        {children}
      </ToolBarDispatchContext.Provider>
    </ToolBarContext.Provider>
  );
};
