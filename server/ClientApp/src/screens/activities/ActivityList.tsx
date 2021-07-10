import React from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { DateTime } from 'luxon';
import { observer } from 'mobx-react';
import { Card, CardContent, List, ListItem, ListItemText, Fab } from '@material-ui/core';
import { Add as AddIcon } from '@material-ui/icons';
import { MainStore } from '../../store/main-store';
import { NavPage } from '../../components/NavPage';

export const SecondaryLines :React.FC<{started: number, units: { id: string, name: string }[]}> = ({started, units}) => {
  const time = new Date().getTime();
  return <>
    {units.length ? <span>{units.map(f => f.name).join(', ')}<br/></span> : null}
    <span>{started < time ? 'Started' : 'Starts'} {DateTime.fromMillis(started).toRelative()}</span>
  </>
}

export const ActivityList: React.FC<{ store: MainStore }> = ({ store }) => {
  const activities = store.activities
  return (
    <NavPage store={store}>
      <Card sx={{ flexGrow: 1 }}>
        <CardContent sx={{flexGrow: 1}}>
          <List>
            {activities.loading ? <div>Loading ...</div> : undefined}
            {activities.obj?.length === 0 ? <ListItem><ListItemText primary="No Recent Activity"/></ListItem> : undefined}
            {activities.obj?.map(a =>
              <ListItem button component={RouterLink} key={a.id} to={`/activity/${a.id}`} style={{borderLeft:`solid 6px ${a.endTime ? '#444' : '#0b0'}`}}>
                <ListItemText primary={(a.number ? `${a.number} `: '') + a.title} secondary={<SecondaryLines started={a.startTime} units={a.units} />} />
              </ListItem>
            )}
          </List>
        </CardContent>
      </Card>
      <Fab color="primary" aria-label="add" component={RouterLink} to="/activity/create" style={{position:'absolute', right: '1rem', bottom: '1rem'}}>
        <AddIcon />
      </Fab>
    </NavPage>
  );
}

export default observer(ActivityList);