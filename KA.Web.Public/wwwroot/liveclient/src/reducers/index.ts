import { combineReducers } from "redux"; 
import auction from './auction';
import user from './user';


const rootReducer = combineReducers({
    auction,
    user
  });
  
// 루트 리듀서를 내보내주세요.
export default rootReducer;


export type RootState = ReturnType<typeof rootReducer>;