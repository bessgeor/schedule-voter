import * as moment from 'moment'

export enum Vote {
	CantAttend = 0,
	MayAttend = 1,
	WantAttend = 2
}
export function voteToString(v: Vote) {
	switch (v) {
		case Vote.CantAttend: return 'Не пойду'
		case Vote.MayAttend: return 'Могу пойти'
		case Vote.WantAttend: return 'Хочу пойти'
	}
}

export type Voting = {
	tourney: moment.Moment
	vote: Vote
}

export type Votes = {
	staticName: string
	gw2User: string
	disUser: string
	votes: Voting[]
}
