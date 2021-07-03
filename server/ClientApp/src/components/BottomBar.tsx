import { BottomNavigation, BottomNavigationAction, Paper } from '@material-ui/core';
import { Link as RouterLink } from 'react-router-dom';
import { observer } from 'mobx-react';
import * as React from 'react';
import { MainStore } from '../store/main-store';

export const BottomBar :React.FC<{store: MainStore}> = ({store}) => {
  let selectedIdx = store.bottomBarActions.length;
  for (let i=0; i<store.bottomBarActions.length; i++) {
    if (store.bottomBarActions[i].isSelected) {
      selectedIdx = i;
      break;
    }
  }
  
  return !store.user.obj ? <></> : (
    <Paper elevation={3}>
      <BottomNavigation
        showLabels
        value={selectedIdx}
      >
        {store.bottomBarActions.map(a => {
          const style = a.isPrimary ? { transform:'scale(1.1)' } : undefined;
          return (<BottomNavigationAction key={a.path} label={a.name} icon={<a.Icon/>} component={RouterLink} to={a.path} style={style} />)
        })}
      </BottomNavigation>
    </Paper>
  );
}

export default observer(BottomBar);