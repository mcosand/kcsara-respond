import { Box, Link } from '@material-ui/core';
import { observer } from 'mobx-react';
import * as React from 'react';
import { NavPage } from '../../../components/NavPage';
import { MainStore } from '../../../store/main-store';
import { MyStatusUiStore } from './myStatusUiStore';

export const MyStatus :React.FC<{ store: MainStore }> = ({store}) => {
  const uiStore = React.useMemo(() => new MyStatusUiStore(store), [store]);
  return (
    <NavPage store={store}>
      {store.belongsToSite
        ? <div>My Status</div>
        : <Box sx={{ m: '1rem'}} style={{border:'solid 1px #800', borderRadius:'.5rem', color:'#800', padding:'.5rem' }}>
            Sorry, our records show you are not a member of {store.branding.name}.
            Please use your unit specific app, or you can try <Link href={`${store.parentSite}me`}>{store.parentSite}</Link> instead.</Box>
      }
    </NavPage>
  );
}

export default observer(MyStatus);