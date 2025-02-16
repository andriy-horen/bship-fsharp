import { Ship } from '@bship/lib/models';
import styled from '@emotion/styled';
import { DraggableBattleship } from './draggable-battleship';

type FleetEditorProps = {
  fleet: Ship[];
};

const Editor = styled.div`
  width: 500px;
  height: 500px;
  outline: 1px solid black;
`;

export const FleetEditor: React.FC<FleetEditorProps> = () => {
  return (
    <Editor>
      <DraggableBattleship ship={[['ship'], ['ship'], ['ship'], ['ship']]} />
    </Editor>
  );
};
