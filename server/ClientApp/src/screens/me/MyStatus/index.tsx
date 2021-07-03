import { observer } from 'mobx-react';
import * as React from 'react';
import { Page } from '../../../components/Page';
import { MainStore } from '../../../store/main-store';

export const MyStatus :React.FC<{ store: MainStore }> = ({store}) => {
  return (
    <Page store={store}>
      <div>My Status</div>
    </Page>
  );
}

export default observer(MyStatus);